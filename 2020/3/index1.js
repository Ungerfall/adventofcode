var fs = require('fs');
var path = require('path');

String.prototype.replaceAt = function(index, replacement) {
    return this.substr(0, index) + replacement + this.substr(index + replacement.length);
}

const inputFile = path.join(__dirname, "input.txt");
let map = fs.readFileSync(inputFile).toString()
    .split("\n");
const right = map[0].length - 1;
const bottom = map.length - 2;
const treeChar = "#";
const toboggan = "O";
const brokenTree = "X";

const traverse = (start, dx, dy) => {
    let treesCount = 0;
    while (true) {
        start.x = start.x + dx > right ? start.x + dx - right - 1 : start.x + dx;
        start.y += dy;
        if (start.y > bottom)
            break;

        if (map[start.y][start.x] === treeChar) {
            treesCount++;
            //map[pos.y] = map[pos.y].replaceAt(pos.x, brokenTree) + ` pos: ${pos.x} ${pos.y}`;
        }
        else {
            //map[pos.y] = map[pos.y].replaceAt(pos.x, toboggan) + ` pos: ${pos.x} ${pos.y}`;
        }
    }

    return (treesCount);
};
let mul = [
    { x: 0, y: 0, dx: 1, dy: 1 },
    { x: 0, y: 0, dx: 3, dy: 1 },
    { x: 0, y: 0, dx: 5, dy: 1 },
    { x: 0, y: 0, dx: 7, dy: 1 },
    { x: 0, y: 0, dx: 1, dy: 2 },
].reduce((acc, v) => {
    const factor = traverse({ x: v.x, y: v.y }, v.dx, v.dy);
    return (acc * factor)
}, 1);

//fs.writeFileSync("output.txt", map.join("\n"));
console.log(mul);