var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
const rules = fs.readFileSync(inputFile).toString()
    .split("\n")
    .filter(s => s.trim().length > 0);

const re = /(.*)bags contain(.*)/;
const valRe = /(\w+ \w+) bag/;
const shiny = "shiny gold";
const no = "no other";
const nodes = rules
    .map(x => {
        const r = x.match(re);
        const key = r[1].trim();
        const children = r[2].split(",").map(c => {
            const m = c.match(valRe);
            return (m[1]);
        });
        const leadToShiny = false;

        return ({key, children, leadToShiny});
    });
const traverse = (node, state) => {
    if (state.stop) {
        return;
    }

    if (node.key == shiny || node.leadToShiny) {
        state.leadToShiny = true;
        state.stop = true;
        return;
    }

    for (let bag of node.children) {
        if (bag == no)
            continue;
        node = nodes.find(x => x.key == bag);
        traverse(node, state);
    }
};

for (let node of nodes) {
    const state = { stop: false, leadToShiny: false };
    traverse(node, state);
    node.leadToShiny = state.leadToShiny;
}

console.log(nodes.filter(x => x.leadToShiny).length - 1);