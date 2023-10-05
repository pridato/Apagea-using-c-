import pyodbc
import json

connection_string = "DRIVER {SQL Server};Server=localhost;Database=dbo;User Id=SA;Password=david2004"
connection = pyodbc.connect(connection_string)

json_path_file = '/Users/davidarroyo/Downloads/libros.json'

with open(json_path_file) as json_file:
    json_data = json.load(json_file)

sql = "INSERT INTO dbo.libros "
"(IdCategoria, ImagenLibro, ImagenLibroBASE64, Titulo, Editorial, "
"Autores, Edicion, NumeroPaginas, Dimensiones, Idioma, ISBN10, ISBN13, Resumen, Precio) "
" VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"

cursor = connection.cursor()
for item in json_data:
    cursor.execute(sql, item['IdCategoria'], item['ImagenLibro'], item['ImagenLibroBASE64'], item['Titulo'], item['Editorial'], item['Autores'],
                   item['Edicion'], item['NumeroPaginas'], item['Dimensiones'], item['Idioma'], item['ISBN10'], item['ISBN13'], item['Resumen'], item['Precio'])
connection.commit()
cursor.close()
