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
        
        // socket.send(JSON.stringify({
        //     type: "position", 
        //     message: JSON.stringify({x: 112, y: 122, tankId: 1})
        // }))
        // TODO send position of other tanks to that user
        // TODO send position of this tank to other users
    }

    handleMessage(socket, data){
        let parsed = JSON.parse(data);

        if(parsed.types == 'position')
        {
            console.log(JSON.parse(parsed.message))
        }

        if(parsed.types == 'fire')
        {
            console.log(JSON.parse(parsed.message))
        }

        if(parsed.types == 'movement')
        {
            console.log(JSON.parse(parsed.message))
        }
        
    }

    //Handle a new connection to the server
    onDisconnected(socket) {
        console.log("client disconnected")
    }
}