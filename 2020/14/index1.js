var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
let init = fs.readFileSync(inputFile).toString()
/*
`mask = 000000000000000000000000000000X1001X
mem[42] = 100
mask = 00000000000000000000000000000000X0XX
mem[26] = 1`
*/
    .split("\n")
    .filter(x => x.trim().length > 0);

const traverse = (sb, floatings, mem, value) => {
    let fli = floatings[floatings.length - 1];
    const rest = floatings.slice(0, floatings.length - 1);
    if (fli == null)
        return;

    let left = sb
        .map((v, i) => i == fli ? 0 : v)
        .map(v => v == "X" ? 0 : v);
    let right = sb
        .map((v, i) => i == fli ? 1 : v)
        .map(v => v == "X" ? 0 : v);

    const leftNum = parseInt(left.join(""), 2);
    const rightNum = parseInt(right.join(""), 2);
    mem[leftNum] = value;
    mem[rightNum] = value;
    //console.log(leftNum, rightNum, value);

    traverse(left, rest, mem, value);
    traverse(right, rest, mem, value);
};

const allocate = (base2, value, mask, mem) => {
    const bits = 36;
    base2 = "0".repeat(bits - base2.length) + base2;
    sb = [];
    for (let i = 0; i < bits; i++) {
        let c = base2.charAt(i);
        let m = mask.charAt(i);
        sb.push(m == "0" ? c : m == "1" ? "1" : "X");
    }

    const floatings = sb
        .map((v, i) => (v == "X" ? i : null))
        .filter(x => x != null);

    traverse(sb, floatings, mem, value);

    return mem;
};

const sum = arr => {
    let sum = 0;
    for (i in arr) {
        if (arr[i] != null)
            sum += arr[i];
    }

    return sum;
}

const re = /^(\w+)(\[\d+\])? = (\w+)/;
let mem = [];
let mask;
for (let i = 0; i < init.length; i++) {
    let r = init[i].match(re);
    if (r[1] == "mask")
        mask = r[3];
    else {
        let adr = +(r[2].match(/\d+/)[0]);
        let value = +(r[3]);
        mem = allocate(adr.toString(2), value, mask, mem);
    }
}

//console.log(mem);
console.log(sum(mem));