var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
const rules = //fs.readFileSync(inputFile).toString()
`shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
dark olive bags contain 3 faded blue bags, 4 dotted black bags.
vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
faded blue bags contain no other bags.
dotted black bags contain no other bags.`
    .split("\n")
    .filter(s => s.trim().length > 0);

const re = /(.*)bags contain(.*)/;
const valRe = /(\d+ )?(\w+ \w+) bag/;
const shiny = "shiny gold";
const no = "no other";
const nodes = rules
    .map(x => {
        const r = x.match(re);
        const key = r[1].trim();
        const children = r[2].split(",").map(c => {
            const m = c.match(valRe);
            const amount = +(m[2] == no ? 0 : m[1]);
            return ({ bag: m[2], amount: amount })
        });

        return ({ key, children });
    });

const traverse = (node, state) => {
    for (let child of node.children) {
        if (child.bag == no || child.bag == shiny) {
            state.sum += 1;
            continue;
        }
        node = nodes.find(x => x.key == child.bag);
        state.sum += child.amount * traverse(node, state);
        console.log(node, state);
    }

    return state.sum;
};

const state = { sum: 0 };
let node = nodes.find(x => x.key == shiny);
const sum = traverse(node, state);

console.log(sum);