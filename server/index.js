let Server = require('./Classes/Server')

var currentPlayers = 0;

const WebSocket = require('ws')

const wss = new WebSocket.Server({ port: process.env.PORT || 3000 },()=>{
    console.log('server started')
})

let server = new Server(wss, process.env.PORT == undefined);

wss.on('connection', (ws) => {
    let connection = server.onConnected(ws)

    let client = server.connectNewClient(ws, currentPlayers)

    // tell other players that new user joined
    wss.broadcast(JSON.stringify({
        type: "init",  
        message: JSON.stringify(
        {
            x: client.x, 
            y: client.z, 
            tankId: client.id
        })
    }),ws)

    currentPlayers++;
    
    ws.on('message', (data) => {
        server.handleMessage(ws, data);
    })

    ws.on('listening',()=>{
        console.log('listening on 3000')
    })

    ws.on('close', ()=>{
        console.log('closed');
        server.disconnectClient(ws)
    }) 
    
})

// broadcast message to all clients without client that sent this message
wss.broadcast = function(data, sender) {
    wss.clients.forEach(function(client) {
        if (client !== sender) {
            client.send(data)
        }
    })
}
