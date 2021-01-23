export const fieldSorter = (fields) => (a, b) => fields.map((o) => {
    let dir = 1;
    if (o[0] === "-") {
        dir = -1; o = o.substring(1); 
    }

    return a[o] > b[o] ? dir : a[o] < b[o] ? -(dir) : 0;
}).reduce((p, n) => (p ? p : n), 0);

export const sortAscedent = (data) => {
    var temp = [];
    for (var x = data.length - 1; 0 <= x; x--) {
        temp.push(data[x]);
    }

    return temp;
}

export const groupBy = (list, keyGetter) => {
    const map = new Map();
    list.forEach((item) => {
         const key = keyGetter(item);
         const collection = map.get(key);
         if (!collection) {
             map.set(key, [item]);
         } else {
             collection.push(item);
         }
    });
    return map;
}