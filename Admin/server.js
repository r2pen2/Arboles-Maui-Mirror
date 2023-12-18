const express = require('express');
const path = require('path');
const cors = require("cors");

const app = express();

app.listen(25565, () => {
    console.log('Now listening on port ' + 25565);
});

app.use(cors());

// Serve React build
app.use(express.static(__dirname + "/arboles-admin/build"));
// Serve react app
app.get("*", (req, res) => {
    res.sendFile(__dirname + "/arboles-admin/build/index.html");
});

app.get("*", (req, res) => {
    res.send("Error: 404!");
});