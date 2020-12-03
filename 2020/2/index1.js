var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
const re = /(\d+)-(\d+) (\w): (\w+)/;
let validPasswordsCount = fs.readFileSync(inputFile).toString()
    .split("\n")
    .filter(s => {
        let result = s.match(re);

        return (
            +(result[3] === result[4][result[1]-1])
            + +(result[3] === result[4][result[2]-1]) === 1
        );
    })
    .length;

console.log(validPasswordsCount);