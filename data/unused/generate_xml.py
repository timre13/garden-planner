with open("szomszedok.txt", encoding="utf-8") as f:
    content = f.read().lower()

content = [x.strip() for x in content.split("\n\n")]

class Plant:
    def __init__(self) -> None:
        self.name = ""
        self.good = []
        self.bad = []

plants = []

for c in content:
    c = c.splitlines()

    p = Plant()
    p.name = c[0][:-1]
    for l in c[1:]:
        if l.startswith("jó szomszéd: "):
            p.good.extend(l.split(":")[1].strip().split(", "))
        if l.startswith("rossz szomszéd: "):
            p.bad.extend(l.split(":")[1].strip().split(", "))
    plants.append(p)


output = "<xml>\n"

for p in plants:
    output += " "*4+f"<plant name=\"{p.name}\">\n"
    for g in p.good:
        output += " "*8+f"<good>{g}</good>\n"
    for b in p.bad:
        output += " "*8+f"<bad>{b}</bad>\n"
    output += " "*4+"</plant>\n"

output += "</xml>\n"

print(output)

with open("neighbours.xml", "w+", encoding="utf-8") as f:
    f.write(output)