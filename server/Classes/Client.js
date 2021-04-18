module.exports = class Client {
    constructor(socket, spawnX, spawnY, id) {
        this.socket = socket;
        this.x = spawnX;
        this.y = spawnY;
        this.id = id; // = tankId
    }
}