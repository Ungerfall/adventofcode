var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
let init = fs.readFileSync(inputFile).toString()
/*
`mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
mem[8] = 11
mem[7] = 101
mem[8] = 0`
*/
    .split("\n")
    .filter(x => x.trim().length > 0);

const applyMask = (base2, mask) => {
    base2 = "0".repeat(36 - base2.length) + base2;
    sb = [];
    for (let i = 0; i < 64; i++) {
        let c = base2.charAt(i);
        let m = mask.charAt(i);
        sb.push(m == "X" ? c : m)
    }

    base2 = sb.join("");
    return parseInt(base2, 2);
};

const re = /^(\w+)(\[\d+\])? = (\w+)/;
let mem = [];
let mask;
for (let i = 0; i < init.length; i++) {
    let r = init[i].match(re);
    if (r[1] == "mask")
        mask = r[3];
    else {
        let adr = r[2].match(/\d+/)[0];
        let base2 = (+r[3]).toString(2);
        mem[adr] = applyMask(base2, mask);
    }
}

const sum = arr => {
    let sum = 0;
    for (let el of arr) {
        if (el != null)
            sum += el;
    }

    return sum;
}

console.log(sum(mem));