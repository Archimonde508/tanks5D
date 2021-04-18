module.exports = class Client {
    constructor(socket, spawnX, spawnZ, id) {
        this.socket = socket;
        this.x = spawnX;
        this.z = spawnZ;
        this.id = id; // = tankId
    }
}