var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
let passports = fs.readFileSync(inputFile).toString()
    .split("\n\n");
const kvRe = /(\w+):(.+)/;
const optional = "cid";
const required = ["byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"];

const validate = (k, v) => {
    if (k == "byr") {
        return (+v >= 1920 && +v <= 2002);
    }
    
    if (k == "iyr") {
        return (+v >= 2010 && +v <= 2020);
    }

    if (k == "eyr") {
        return (+v >= 2020 && +v <= 2030);
    }

    if (k == "hgt") {
        const r = v.match(/(\d+)(cm|in)/);
        if (r == null) {
            return false;
        }

        return (
            r[2] == "cm"
            ? +r[1] >= 150 && +r[1] <= 193
            : r[2] == "in"
                ? +r[1] >= 59 && +r[1] <= 76
                : false
        );
    }

    if (k == "hcl") {
        return (v.trim().match(/#(\d|[a-f]){6}$/) != null)
    }

    if (k == "ecl") {
        return (["amb", "blu", "brn", "gry", "grn", "hzl", "oth"].includes(v));
    }

    if (k == "pid") {
        return (v.trim().match(/^\d{9}$/) != null);
    }

    return true;
};

const valid = passports
    .filter(x => {
        const kv = x
            .split(/\n| /)
            .filter(e => e.trim().length > 0)
            .map(s => {
                const r = s.match(kvRe);
                return({ key: r[1], v: r[2] });
            })
            .filter(m => required.includes(m.key) && validate(m.key, m.v));

        return (kv.length >= 7);
    });

console.log(valid.length);