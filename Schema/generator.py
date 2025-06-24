import os
import sys
import xml.etree.ElementTree as ET

INDENT = "\t"
NEWLINE = "\n"
OUTPUT_PATH = "../Client/Data/Generated.cs"
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
            if precision <= 4:
                return "short"
            elif precision <= 9:
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
        yield f"///{table_comment}"
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
            yield f"{INDENT}///{comment}"
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
    yield "}"
    yield ""
    yield f"public partial class {class_name}Table"
    yield "{"
    yield "}"

xmlFile = sys.argv[1]
root = ET.parse(xmlFile).getroot()

output_file = os.path.abspath(OUTPUT_PATH)

dbsets = []
classes = []

for db in root.findall('Database'):
    for schema in db.findall('Schema'):
        for table in schema.findall('Table'):
            class_name = table.get("ClassName")
            repository_name = table.get("RepositoryName")
            comment = table.get("Comment")
            if comment:
                dbsets.append(f"{INDENT}/// <summary>")
                dbsets.append(f"{INDENT}///{comment}")
                dbsets.append(f"{INDENT}/// </summary>")
            dbsets.append(f"{INDENT}public static {class_name}Table {repository_name} = new {class_name}();")
            classes.extend(get_class_lines(table))
content_lines = [
    "using System;",
    "using Grpc.Core;",
    
    "using Microsoft.EntityFrameworkCore;",
    "using Client;",
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