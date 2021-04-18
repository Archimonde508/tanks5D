module.exports = class Server {
    constructor(wss, isLocal = false) {
        let server = this;
        this.clients = {}; // {socket: id}
        this.websocketServer = wss;
    }

    addNewClient(socket, id) {
        this.clients[socket] = id
    }

    //Interval update every 100 miliseconds
    onUpdate() {
        
    }

    //Handle a new connection to the server
    onConnected(socket) {
        console.log("client connected")
        
        // socket.send(JSON.stringify({
        //     type: "position", 
        //     message: JSON.stringify({x: 112, y: 122, tankId: 1})
        // }))
        // TODO send position of other tanks to that user
        // TODO send position of this tank to other users
    }

    validateMessage(id)
    {
        // TODO token validation of message
        return true;
    }

    handleMessage(socket, data){
        let parsed = JSON.parse(data);
        console.log('parsed', parsed)

        // tankId = parsed.id
        if(!this.validateMessage(parsed.id))
            return;

        if(parsed.type == 'position')
        {
            console.log(JSON.parse(parsed.message))
        }

        if(parsed.type == 'fire')
        {
            console.log(JSON.parse(parsed.message))
        }

        if(parsed.type == 'movement')
        {
            console.log(JSON.parse(parsed.message))
            this.websocketServer.broadcast(data, socket);
        }
        
    }

    //Handle a new connection to the server
    onDisconnected(socket) {
        console.log("client disconnected")
    }
}