var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
const re = /(\d+)-(\d+) (\w): (\w+)/;
let validPasswordsCount = fs.readFileSync(inputFile).toString()
    .split("\n")
    .filter(s => {
        let result = s.match(re);
        let occurences = result[4].split(result[3]).length - 1;

        return (parseInt(result[1]) <= occurences && occurences <= parseInt(result[2]));
    })
    .length;

console.log(validPasswordsCount);