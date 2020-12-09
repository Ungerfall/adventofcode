var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
const instructions = fs.readFileSync(inputFile).toString()
        .split("\n")
        .filter(s => s.trim().length > 0)
        .map(x => {
            let m = x.match(/(\w+) ([+-]\d+)/);
            return ({ opCode: m[1], num: +m[2], visited: false, new: false });
        });

let acc = 0;
let i = 0
let curr = instructions[i];
let history = [{ in: curr, i, acc }];
while (true) {
    if (curr.visited) {
        let prev = history.pop();
        while (true) {
            if (prev.in.opCode == "acc" || prev.in.new)
                prev = history.pop();
            else
                break;
        }

        curr = prev.in;
        curr.opCode = curr.opCode == "jmp" ? "nop" : "jmp";
        curr.visited = false;
        curr.new = true;
        i = prev.i;

        instructions[prev.i] = curr;
        acc = prev.acc;

        continue;
    }

    history.push({ in: curr, i, acc})
    curr.visited = true;
    switch (curr.opCode) {
        case "nop":
            curr = instructions[++i];
            break;
        case "acc":
            acc += curr.num;
            curr = instructions[++i];
            break;
        case "jmp":
            i += curr.num;
            curr = instructions[i];
            break;
        default:
            console.log(`default in switch, ${curr.opCode}`);
    }

    if (i >= instructions.length)
        break;
}

console.log(acc);
