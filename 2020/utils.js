const sum = arr => {
    let sum = 0;
    for (let el of arr) {
        sum += el;
    }

    return sum;
}

module.exports = {
    sum,
}