import os
import sys
import xml.etree.ElementTree as ET

INDENT = "\t"
NEWLINE = "\n"
OUTPUT_PATH = "../OrmFactoryCom/Model/DataContextGenerated.cs"
NAMESPACE = "OrmFactoryCom.Model"
CONTEXT_NAME = "DataContext"

def is_database_generated(column: ET.Element):
    default_value = column.get("Default")
    return default_value == "current_timestamp()"

def resolve_type(db_type: str) -> str:
    if db_type.startswith("tinyint(1)"):
        return "bool"
    if any(db_type.startswith(t) for t in ["tinyint", "smallint", "mediumint", "int"]):
        if "unsigned" in db_type:
            return "uint"
        return "int"
    if db_type.startswith("bigint"):
        if "unsigned" in db_type:
            return "ulong"
        return "long"
    if any(db_type.startswith(t) for t in ["timestamp", "datetime"]):
        return "DateTime"
    if any(db_type.startswith(t) for t in ["varchar", "char", "tinytext", "mediumtext", "text", "longtext", "set", "enum", "geometry"]):
        return "string"
    if db_type.startswith("year"):
        return "int"
    if db_type.startswith("date"):
        return "DateOnly"
    if db_type.startswith("decimal"):
        return "decimal"
    if db_type.startswith("blob"):
        return "byte[]"
    raise ValueError(f"Unknown type: {db_type}")

def resolve_type_from_column(column: ET.Element) -> str:
    db_type = column.get("DatabaseType").lower()
    nullable = column.get("Nullable", "false").lower() == "true"
    resolved_type = resolve_type(db_type)
    if nullable:
        return resolved_type + "?"
    return resolved_type

def get_class_lines(table: ET.Element):
    table_name = table.get("Name")
    class_name = table.get("ClassName")
    table_comment = table.get("Comment")
    
    yield ""
    if table_comment:
        yield f"{INDENT}/// <summary>"
        yield f"{INDENT}///{table_comment}"
        yield f"{INDENT}/// </summary>"
    #if table_name != class_name:
    yield f"[Table(\"{table_name}\")]"
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
        if column.get("PrimaryKey"):
            yield f"{INDENT}[Key]"
        elif is_database_generated(column):
            yield f"{INDENT}[DatabaseGenerated(DatabaseGeneratedOption.Computed)]"
        if column_name != field_name:
            yield f"{INDENT}[Column(\"{column_name}\")]"
        csharp_type = resolve_type_from_column(column)
        yield f"{INDENT}public {csharp_type} {field_name} {{ get; set; }}"
    for fk in table.findall("ForeignKey"):
        from_col = fk.get('FromColumn')
        field_name = fk.get('FieldName')
        class_type = fk.get('ToClassName')
        from_column = columns_dict[from_col]
        from_field_name = from_column.get("FieldName")
        is_nullable = from_column.get("Nullable", "false").lower() == "true"
        yield f"{INDENT}[ForeignKey(\"{from_field_name}\")]"
        if is_nullable:
            class_type = class_type + "?"
        yield f"{INDENT}public {class_type} {field_name} {{ get; set; }}"
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
            dbsets.append(f"{INDENT}public DbSet<{class_name}> {repository_name} {{ get; set; }}")
            classes.extend(get_class_lines(table))
content_lines = [
    "using System;",
    "using System.ComponentModel.DataAnnotations;",
    "using System.ComponentModel.DataAnnotations.Schema;",
    "using Microsoft.EntityFrameworkCore;",
    "",
    f"namespace {NAMESPACE};",
    "",
    f"public partial class {CONTEXT_NAME} : DbContext",
    "{"]
content_lines.extend(dbsets)
content_lines.append("}")
content_lines.extend(classes)

content = NEWLINE.join(content_lines)
with open(output_file, "w", encoding="utf-8", newline=NEWLINE) as f:
    f.write(content)