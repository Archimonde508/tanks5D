let Server = require('./Classes/Server')



const WebSocket = require('ws')
const PORT = process.env.PORT || 3000;

const wss = new WebSocket.Server({ port: PORT },()=>{
    console.log('server started')
    console.log('listeing on port '+PORT)
})


let server = new Server(wss, process.env.PORT == undefined);

wss.on('connection', (ws) => {

    function sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    let connection = server.onConnected(ws)

    let client = server.connectNewClient(ws)

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
    
    server.incrementCurrentplayersBecuaseJsIsShit();

    console.log("Liczba graczy = " + server.getCurrentPlayers());

    if(server.getCurrentPlayers() == 2){ // powinno reagowac na opa gry rozpoczynajacego
        console.log("HALO ZARAZ ZACZNIEMY");
        sleep(2000).then(() => { server.zacznijGre(ws) });
    }

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
