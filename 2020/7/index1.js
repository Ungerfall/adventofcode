var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
const rules = fs.readFileSync(inputFile).toString()
        .split("\n")
        .filter(s => s.trim().length > 0);

const re = /(.*)bags contain(.*)/;
const valRe = /(\d+) (\w+ \w+) bag/;
const noRe = /\s*no other bag.*/;
const shiny = "shiny gold";
const nodes = rules
    .map(x => {
        const r = x.match(re);
        const key = r[1].trim();
        const children = r[2].match(noRe)
            ? null
            : r[2].split(",").map(c => {
                const m = c.match(valRe);
                return ({ bag: m[2], amount: +m[1] })
            });

        return ({ key, children });
    });

const traverse = (node, state) => {
    if (node.children == null) {
        return 0;
    }

    let sum = 0;
    for (let child of node.children) {
        if (child.bag == shiny)
            continue;

        node = nodes.find(x => x.key == child.bag);
        sum += child.amount + child.amount * traverse(node, state);
    }

    return sum;
};

const state = { sum: 0 };
let node = nodes.find(x => x.key == shiny);
const sum = traverse(node, state);

console.log(sum);