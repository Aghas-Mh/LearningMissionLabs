import React, { useState, useEffect } from 'react';
import MyInput from './UI/input/MyInput';
import { Service } from '../Service';
import ChatUserModel from './ChatUserModel';
import MyButton from './UI/button/MyButton';
import MessageForm from './MessageForm';
import { jwtDecode } from "jwt-decode";
import MyModel from './UI/MyModel/MyModel'

const Chat = () => {
    const [users, setUsers] = useState([{}])  // список всех пользователей
    const [messages, setMessages] = useState([{}])  // список моих сообщений
    const [sendingMessage, setSendingMessage] = useState('')  // отправляемое сообщение
    const [current, setCurrent] = useState(0)  // текущий пользователь, чей чат открит
    const [currentMessages, setCurrentMessages] = useState([]) // список сообщений текущего чата
    const [myEmail, setMyEmail] = useState('')
    const [model, setModel] = useState(false)
    const [addGroupModel, setAddGroupModel] = useState(false)
    const [currAddUserName, setName] = useState('')
    const [addedList, setAddedList] = useState([])
    const [groupName, setGroupName] = useState('')

    useEffect(() => {  // отправляем запрос на получение пользователей и своих сообщений
        getUsers()
        getMyMessages()
    }, [])

    // метод получения емайл-а из токена
    async function setEmail(token)
    {
        const decoded = jwtDecode(token);
        const email = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']
        setMyEmail(email)
    }

    // Метод получения всех пользователей
    async function getUsers()
    {
        const token = Service.getToken()
        setEmail(token)
        const url = Service.getServerUrl()
        const requestOptions = {
            method: 'GET',
            headers: { 'Accept-Type': 'application/json', 'Authorization': `bearer ${token}` }
        }
        const response = await fetch(`${url}/User/GetAllUsers`, requestOptions)
        const usersList = await response.json()
        console.log(usersList)
        setUsers(usersList)
    }

    // Метод получения своих сообщений
    async function getMyMessages()
    {
        const token = Service.getToken()
        const url = Service.getServerUrl()
        const requestOptions = {
            method: 'GET',
            headers: { 'Accept-Type': 'application/json', 'Authorization': `bearer ${token}` }
        }
        const response = await fetch(`${url}/User/MyMessages`, requestOptions)
        const myMessages = await response.json()
        myMessages.map(async jsonData => {
            jsonData.message = await Service.decryptMessage(jsonData.message)
        })
        setMessages(myMessages)
    }

    // Метод отправки сообщения
    // Сообщение перед отправкой шифруется
    async function Send()
    {
        const token = Service.getToken()
        const url = Service.getServerUrl()
        const encryptedMessage = await Service.encryptMessage(sendingMessage)
        setSendingMessage('')
        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'Authorization': `bearer ${token}` },
            body: JSON.stringify({
                'reciver': users[current].email,
                'message': encryptedMessage
            })
        }
        const response = await fetch(`${url}/User/Message`, requestOptions)
        const result = await response.text()
        console.log("Send Result: ", result)
    }

    // Вспомогательний метод, для настройки текущего чата
    function callback(index)
    {
        setCurrent(index)
        const currentEmail = users[index].email
        var messagesToView = []
        messages.map(message => {
            if (message.sender === currentEmail || message.reciver === currentEmail) {
                messagesToView.push(message)
            }
        })
        setCurrentMessages(messagesToView)
    }

    function toGroupModel()
    {
        setModel(false)
        setAddGroupModel(true)
    }

    async function CreateGroup()
    {
        const url = Service.getServerUrl()
        const token = Service.getToken()
        const createData = JSON.stringify({'groupName': groupName, 'users': addedList})
        console.log('CREATE DATA: ', createData)
        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'Authorization': `bearer ${token}` },
            body: createData
        }
        const response = await fetch(`${url}/User/CreateGroup`, requestOptions)
        const message = await response.text()
        console.log(message)
    }

    function addName(e)
    {
        if (e.key === 'Enter')
        {
            add()
        }
    }

    function add()
    {
        if (currAddUserName !== '')
            setAddedList([...addedList, currAddUserName])
        setName('')
    }

    return (
        <div style={{height: window.innerHeight * 85 / 100}}>
            <MyModel visible={model} setVisible={setModel}>
                <MyButton style={{width: 50, margin: 10, float: 'left'}}>User</MyButton>
                <MyButton style={{width: 50, margin: 10, float: 'right'}} onClick={toGroupModel}>Group</MyButton>
            </MyModel>
            <MyModel visible={addGroupModel} setVisible={setAddGroupModel}>
                <MyInput placeholder='Group Name' value={groupName} onChange={e => setGroupName(e.target.value)} />
                <div>Users: {addedList.map((name, index) => <a key={index}>{name}, </a>)}</div>
                <MyButton style={{float: 'right', width: '9%'}} onClick={add}>+</MyButton>
                <MyInput style={{float: 'left', width: '90%'}} placeholder='Users' value={currAddUserName} onChange={e => setName(e.target.value)} onKeyDown={addName}/>
                <MyButton onClick={CreateGroup}>Create</MyButton>
            </MyModel>
            <div className='UsersList' style={{border: "2px solid black", float: 'left', borderRadius: 10, width: '25%', height: '99%', backgroundColor: 'white', padding: 10}}>
                <div className='Add' onClick={() => setModel(true)} style={{height: 30, width: 30, border: '2px solid black', borderRadius: '50%', color: 'black', float: 'Right', fontSize: 14, paddingTop: 2}}>Add</div>
                <div>{myEmail}</div>
                <MyInput placeholder='search' />
                {users.map((user, index) =>
                    <ChatUserModel user={user} index={index} key={index} callback={callback}/>
                )}
            </div>
            <div style={{border: '2px solid black', float:'right', borderRadius: 10, height: '99%', width: '73%', backgroundColor: 'white'}}>
                <div style={{paddingLeft: 5, textAlign: 'center', backgroundColor: 'rgb(163, 161, 187)', borderRadius: 8}}>{users[current].email}</div>
                <div className='Messages' style={{float:'right', borderRadius: 10, height: '89%', width: '100%', backgroundColor: 'white', padding: 3}}>
                    {currentMessages.map((messageForm, index) => 
                        <MessageForm messageForm={messageForm}  myEmail={myEmail} key={index}>
                        </MessageForm> 
                    )}
                </div>
                <MyButton onClick={Send} dynamicSize={true} style={{float: 'right', width: '9%', backgroundColor: 'white'}}>Send</MyButton>
                <MyInput
                    style={{ float: 'left', width: '90%' }}
                	value={sendingMessage}
                    type="text"
                    placeholder='Message'
                    onChange={e => setSendingMessage(e.target.value)}
                />
            </div>
        </div>
    )
}

export default Chat;