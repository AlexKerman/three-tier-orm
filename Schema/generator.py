import os
import sys
import xml.etree.ElementTree as ET

INDENT = "\t"
NEWLINE = "\n"
OUTPUT_PATH = "../Client/Data/Generated.cs"
PROTO_PATH = "../Shared/GrpcContracts/Generated.proto"
NAMESPACE = "Client.Data"

def is_database_generated(column: ET.Element):
    default_value = column.get("Default")
    return default_value == "current_timestamp()"

def resolve_type(db_type: str, column_name: str) -> str:
    db_type = db_type.lower().strip()

    if db_type.startswith("number"):    
        if column_name.endswith("ID"):
            return "int"
    
        precision = None
        scale = None

        if "(" in db_type and ")" in db_type:
            args = db_type[db_type.find("(") + 1 : db_type.find(")")]
            parts = [p.strip() for p in args.split(",")]
            if len(parts) >= 1 and parts[0].isdigit():
                precision = int(parts[0])
            if len(parts) == 2 and parts[1].isdigit():
                scale = int(parts[1])
            elif len(parts) == 1:
                scale = 0  # default if only precision is given

        if scale is not None and scale > 0:
            return "decimal"
        if precision is not None:
            if precision <= 9:
                return "int"
            elif precision <= 18:
                return "long"
            else:
                return "decimal"
        return "decimal"

    if db_type.startswith("float") or db_type.startswith("binary_float"):
        return "float"
    if db_type.startswith("binary_double"):
        return "double"
    if any(db_type.startswith(t) for t in ["varchar2", "nvarchar2", "char", "nchar", "clob", "nclob"]):
        return "string"
    if db_type.startswith("timestamp"):
        return "DateTime"
    if db_type.startswith("date"):
        return "DateOnly"
    if any(db_type.startswith(t) for t in ["blob", "raw", "long raw"]):
        return "byte[]"

    raise ValueError(f"Unknown type: {db_type}")

def resolve_proto_type(db_type: str, column_name: str, nullable: bool):
    csharp_type = resolve_type(db_type, column_name)
    if csharp_type == "string":
        return "optional string" if nullable else "string"
    if csharp_type == "int":
        return "optional int32" if nullable else "int32"
    if csharp_type == "long":
        return "optional int32" if nullable else "int64"
    if csharp_type == "decimal":
        return "DecimalProto"
    if csharp_type == "float":
        return "optional float" if nullable else "float"
    if csharp_type == "double":
        return "optional double" if nullable else "double"
    if csharp_type == "DateTime":
        return "optional double" if nullable else "double"
    if csharp_type == "DateOnly":
        return "DateProto"
    if csharp_type == "byte[]":
        return "bytes"
        
    raise ValueError(f"Unknown type: {db_type}")

def resolve_type_from_column(column: ET.Element) -> str:
    db_type = column.get("DatabaseType").lower()
    column_name = column.get("Name")
    nullable = column.get("Nullable", "false").lower() == "true"
    resolved_type = resolve_type(db_type, column_name)
    if nullable:
        return resolved_type + "?"
    return resolved_type

def get_class_lines(table: ET.Element):
    table_name = table.get("Name")
    class_name = table.get("ClassName")
    table_comment = table.get("Comment")
    
    yield ""
    if table_comment:
        yield f"/// <summary>"
        yield f"/// {table_comment}"
        yield f"/// </summary>"

    yield f"public partial class {class_name}"
    yield "{"
    columns_dict = {}
    for column in table.findall("Column"):
        column_name = column.get("Name")
        field_name = column.get("FieldName")
        comment = column.get("Comment")
        columns_dict[column_name] = column
        if comment:
            yield f"{INDENT}/// <summary>"
            yield f"{INDENT}/// {comment}"
            yield f"{INDENT}/// </summary>"
        csharp_type = resolve_type_from_column(column)
        yield f"{INDENT}public {csharp_type} {field_name} {{ get; set; }}"
    for fk in table.findall("ForeignKey"):
        from_col = fk.get('FromColumn')
        field_name = fk.get('FieldName')
        class_type = fk.get('ToClassName')
        from_column = columns_dict[from_col]
        from_field_name = from_column.get("FieldName")
        is_nullable = from_column.get("Nullable", "false").lower() == "true"
        if is_nullable:
            class_type = class_type + "?"
        yield f"{INDENT}public {class_type} {field_name} {{ get; set; }}"
    yield ""
    yield from get_from_proto_constructor(table)
    yield from get_get_proto(table)
    yield "}"
    yield ""
    yield f"public partial class {class_name}Table : TableBase<{class_name}>"
    yield "{"
    yield "}"

def get_proto_rpc(table: ET.Element):
    class_name = table.get("ClassName")
    yield f"{INDENT}rpc Select{class_name} (SelectRequest) returns (Select{class_name}Reply);"
    yield f"{INDENT}rpc Select{class_name}Stream (SelectRequest) returns (stream {class_name}Proto);"

def get_proto_entities(table: ET.Element):
    class_name = table.get("ClassName")
    yield f"message Select{class_name}Reply {{"
    yield f"{INDENT}repeated {class_name}Proto Objects = 1;"
    yield f"{INDENT}int32 ErrorCode = 2;"
    yield f"{INDENT}string ErrorMessge = 3;"
    yield "}"
    yield ""
    yield f"message {class_name}Proto {{"
    i = 1
    for column in table.findall("Column"):
        column_name = column.get("Name")
        field_name = column.get("FieldName")
        db_type = column.get("DatabaseType").lower()
        nullable = column.get("Nullable", "false").lower() == "true"
        proto_type = resolve_proto_type(db_type, column_name, nullable)
        yield f"{INDENT}{proto_type} {field_name} = {i};"
        i = i + 1
        
    for fk in table.findall("ForeignKey"):
        from_col = fk.get('FromColumn')
        field_name = fk.get('FieldName')
        class_name = fk.get('ToClassName')
        yield f"{INDENT}{class_name}Proto {field_name} = {i};"
        i = i + 1
    yield "}"
    yield ""

def get_convert_field_from_proto(proto_type: str, from_expr: str):
    if proto_type == "DateProto":
        return f"DateProto.ToDateOnly({from_expr})"
    if proto_type == "DecimalProto":
        return f"DecimalProto.ToDecimal({from_expr})"
    return from_expr

def get_convert_field_to_proto(proto_type: str, from_expr: str):
    if proto_type == "DateProto":
        return f"DateProto.FromDateOnly({from_expr})"
    if proto_type == "DecimalProto":
        return f"DecimalProto.FromDecimal({from_expr})"
    return from_expr

def get_from_proto_constructor(table: ET.Element):
    class_name = table.get("ClassName")
    yield INDENT + f"public {class_name}() {{ }}"
    yield INDENT + f"public {class_name}({class_name}Proto proto)"
    yield INDENT + "{"
    for column in table.findall("Column"):
        field_name = column.get("FieldName")
        column_name = column.get("Name")
        db_type = column.get("DatabaseType").lower()
        nullable = column.get("Nullable", "false").lower() == "true"
        proto_type = resolve_proto_type(db_type, column_name, nullable)
        line = f"{field_name} = {get_convert_field_from_proto(proto_type, "proto." + field_name)};"
        if proto_type.startswith("optional"):
            line = f"if (proto.Has{field_name}) {line}"
        elif nullable:
            line = f"if (proto.{field_name} != null) {line}"
        yield INDENT*2 + line
        
    for fk in table.findall("ForeignKey"):
        from_col = fk.get('FromColumn')
        field_name = fk.get('FieldName')
        class_name = fk.get('ToClassName')
        yield f"{INDENT*2}if (proto.{field_name} != null)"
        yield INDENT*2 + "{"
        yield f"{INDENT*3}{field_name} = new {class_name}(proto.{field_name});"
        yield INDENT*2 + "}"
        
    yield "}"
    yield ""

def get_get_proto(table: ET.Element):
    class_name = table.get("ClassName")
    yield INDENT + f"public {class_name}Proto GetProto()"
    yield INDENT + "{"
    yield f"{INDENT*2}var proto = new {class_name}Proto();"
    for column in table.findall("Column"):
        field_name = column.get("FieldName")
        column_name = column.get("Name")
        db_type = column.get("DatabaseType").lower()
        nullable = column.get("Nullable", "false").lower() == "true"
        proto_type = resolve_proto_type(db_type, column_name, nullable)
        line = f"proto.{field_name} = {get_convert_field_to_proto(proto_type, field_name)};"
        if proto_type.startswith("optional"):
            add_value = ".Value"
            if proto_type == "optional string":
                add_value = ""
            line = f"if ({field_name} != null) proto.{field_name} = {get_convert_field_to_proto(proto_type, field_name)}{add_value};"
        yield INDENT*2 + line
    for fk in table.findall("ForeignKey"):
        from_col = fk.get('FromColumn')
        field_name = fk.get('FieldName')
        class_name = fk.get('ToClassName')
        yield f"{INDENT*2}if ({field_name} != null) proto.{field_name} = {field_name}.GetProto();"
    yield f"{INDENT*2}return proto;"
    yield INDENT + "}"
    yield ""
    
xmlFile = sys.argv[1]
root = ET.parse(xmlFile).getroot()

output_file = os.path.abspath(OUTPUT_PATH)

dbsets = []
classes = []
rpc = []
proto = []

for db in root.findall('Database'):
    for schema in db.findall('Schema'):
        for table in schema.findall('Table'):
            class_name = table.get("ClassName")
            repository_name = table.get("RepositoryName")
            comment = table.get("Comment")
            if comment:
                dbsets.append(f"{INDENT}/// <summary>")
                dbsets.append(f"{INDENT}/// {comment}")
                dbsets.append(f"{INDENT}/// </summary>")
            dbsets.append(f"{INDENT}public static {class_name}Table {repository_name} = new();")
            classes.extend(get_class_lines(table))
            rpc.extend(get_proto_rpc(table))
            proto.extend(get_proto_entities(table))
content_lines = [
    "using System;",
    "using Grpc.Core;",
    "using Client;",
    "using GrpcContracts;",
    "",
    f"namespace {NAMESPACE};",
    "",
    f"public static class Tables",
    "{"]
content_lines.extend(dbsets)
content_lines.append("}")
content_lines.extend(classes)

content = NEWLINE.join(content_lines)
with open(output_file, "w", encoding="utf-8", newline=NEWLINE) as f:
    f.write(content)
    
proto_lines = [
    "syntax = \"proto3\";",
    "option csharp_namespace = \"GrpcContracts\";",
    "import \"Common.proto\";",
    "import \"google/protobuf/timestamp.proto\";",
    "package orm;",
    "",
    "service Orm {"
    ]
proto_lines.extend(rpc)
proto_lines.extend("}")
proto_lines.extend([""])
proto_lines.extend(proto)

output_file = os.path.abspath(PROTO_PATH)
content = NEWLINE.join(proto_lines)
with open(output_file, "w", encoding="utf-8", newline=NEWLINE) as f:
    f.write(content)