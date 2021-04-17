module.exports = class Server {
    constructor(isLocal = false) {
        let server = this;    
    }

    //Interval update every 100 miliseconds
    onUpdate() {
        
    }

    //Handle a new connection to the server
    onConnected(socket) {
        console.log("client connected")
    }
}