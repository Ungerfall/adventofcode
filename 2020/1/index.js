var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
var numbers = fs.readFileSync(inputFile).toString()
    .split("\n")
    .map(v => parseInt(v));
const required = 2020;

for(i in numbers) {
    for(j in numbers) {
        if (numbers[i] + numbers[j] == required) {
            console.log(numbers[i] * numbers[j]);
        }
    }
}