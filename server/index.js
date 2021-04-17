// let io = require('socket.io')(process.env.PORT || 3000);
let Server = require('./Classes/Server')
let server = new Server(process.env.PORT == undefined);

const WebSocket = require('ws')
const wss = new WebSocket.Server({ port: 3000 },()=>{
    console.log('server started')
})
wss.on('connection', function connection(ws) {
    let connection = server.onConnected(ws);
    ws.send(JSON.stringify({method: "init", message: "{}"}))
    // console.log(connection)
    ws.on('message', (data) => {
        server.handleMessage(ws, data);
        console.log('data received \n %o',data)
        // ws.send(data);
    })

    ws.on('listening',()=>{
        console.log('listening on 3000')
    })

    ws.on('disconnect', (ws)=>{
        console.log('disconnected');
    }) 
    
})

// wss.on('message', (data) => {
//     console.log('data received \n %o',data)
//     ws.send(data);
// })

// wss.on('listening',()=>{
//    console.log('listening on 3000')
// })

// wss.on('disconnect', (ws)=>{
//     server.onDisconnected(ws);
// }) 

// console.log('Server has started');

// if (process.env.PORT == undefined) {
//     console.log('Local Server');
// } else {
//     console.log('Hosted Server');
// }

// let server = new Server(process.env.PORT == undefined);

// setInterval(() => {
//     server.onUpdate();
// }, 100, 0);

// io.on('connection', (socket) => {
//     console.log(socket)
//     let connection = server.onConnected(socket);
//     connection.createEvents();
//     connection.socket.emit('register', {'id': connection.player.id});
// });