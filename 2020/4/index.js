var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
let passports = fs.readFileSync(inputFile).toString()
    .split("\n\n");
const kvRe = /(\w+):(.+)/;
const optional = "cid";
const required = ["byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"];
const valid = passports
    .filter(x => {
        const kv = x
            .split(/\n| /)
            .filter(e => e.trim().length > 0)
            .map(s => {
                const r = s.match(kvRe);
                return({ key: r[1], v: r[2] });
            })
            .filter(m => required.includes(m.key));

        return (kv.length >= 7);
    });

console.log(valid.length);