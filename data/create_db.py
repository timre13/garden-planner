import xml.etree.ElementTree as ET

tree = ET.parse("neighbours.xml")

root = tree.getroot()

class Plant:
    def __init__(self, name, sortav, totav):
        self.name = name
        self.sortav = sortav
        self.totav = totav

plants = set()
goodPairs = set()
badPairs = set()

for child in root:
    plants.add(Plant(
        child.attrib["name"],
        child.attrib.get("sortav", 0),
        child.attrib.get("totav", 0)))
    for child1 in child:
        if child1.tag == "good":
            goodPairs.add(".".join(sorted([child.attrib["name"], child1.text])))
        elif child1.tag == "bad":
            badPairs.add(".".join(sorted([child.attrib["name"], child1.text])))


import sqlite3
con = sqlite3.connect("database.db")
cur = con.cursor()

cur.execute("DROP TABLE IF EXISTS plants")
cur.execute("""
    CREATE TABLE plants (
        id INTEGER UNIQUE NOT NULL PRIMARY KEY,
        name TEXT UNIQUE NOT NULL,
        sortavolsag INTEGER NOT NULL,
        totavolsag INTEGER NOT NULL,
        color STRING
    )
""")

for plant in tuple(plants):
    cur.execute("""
    INSERT INTO plants (name, sortavolsag, totavolsag, color)
    VALUES (?, ?, ?, ?)""", [plant.name, plant.sortav, plant.totav, "#33dd55"])

cur.execute("DROP TABLE IF EXISTS good_neighbours")
cur.execute("""
    CREATE TABLE good_neighbours (
        plant1 INTEGER NOT NULL,
        plant2 INTEGER NOT NULL,
        FOREIGN KEY (plant1) REFERENCES plants(id),
        FOREIGN KEY (plant2) REFERENCES plants(id)
    )
""")

for pair in goodPairs:
    plant1, plant2 = pair.split(".")
    cur.execute("SELECT id FROM plants WHERE name = ?", [plant1])
    try:
        id1 = cur.fetchone()[0]
    except:
        continue
    cur.execute("SELECT id FROM plants WHERE name = ?", [plant2])
    try:
        id2 = cur.fetchone()[0]
    except:
        continue
    cur.execute("""
        INSERT INTO good_neighbours (plant1, plant2)
        VALUES (?, ?)
    """, [id1, id2])

cur.execute("DROP TABLE IF EXISTS bad_neighbours")
cur.execute("""
    CREATE TABLE bad_neighbours (
        plant1 INTEGER NOT NULL,
        plant2 INTEGER NOT NULL,
        FOREIGN KEY (plant1) REFERENCES plants(id),
        FOREIGN KEY (plant2) REFERENCES plants(id)
    )
""")

for pair in badPairs:
    plant1, plant2 = pair.split(".")
    cur.execute("SELECT id FROM plants WHERE name = ?", [plant1])
    try:
        id1 = cur.fetchone()[0]
    except:
        continue
    cur.execute("SELECT id FROM plants WHERE name = ?", [plant2])
    try:
        id2 = cur.fetchone()[0]
    except:
        continue
    cur.execute("""
        INSERT INTO bad_neighbours (plant1, plant2)
        VALUES (?, ?)
    """, [id1, id2])

con.commit()
con.close()