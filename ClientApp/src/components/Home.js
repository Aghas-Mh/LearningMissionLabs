import React, { useState, useEffect, useMemo } from 'react';
import MyModel from './UI/MyModel/MyModel'
import PostForm from './PostForm';
import MyButton from './UI/button/MyButton';
import GroupedPosts from './GroupedPosts';
import { Service } from '../Service';
import { useHistory } from 'react-router-dom/cjs/react-router-dom.min';
import { Link } from 'react-router-dom';

const Home = () => {
    const [modal, setModel] = useState(false)   // модель создания нового поста
    const [infoModel, setInfoModel] = useState(false)   // модель инфромации о себе
    
    const [posts, setPosts] = useState([])  // Список новостей
    const [elementInGroup, setGroup] = useState(4)  // Количество новостей в одном ряду, динамически меняется, в зависимости от размера окна
    const [info, setInfo] = useState({})    // инфромация о себе
    
    const history = useHistory()
    useMemo(() => {
        const token = Service.getToken()
        if (!token) {
            alert('You are not logged in. Please login before going to this page')
            history.push('/LoginForm')
        } else {
            pullPosts()
        }
    }, []);

    // Подключение/отключение слушателя изменений размера окна браузера
    useEffect(() => {
        window.addEventListener('resize', windowSizeHandler)
        return () => {
            window.removeEventListener('resize', windowSizeHandler)
        }
    }, [elementInGroup])

    // Изменяет количество новостних постов в одном ряду, в зависимости от размера окна
    async function windowSizeHandler() {
        if (window.innerWidth > 992) {
            if (elementInGroup !== 4) setGroup(4)
        } else if (window.innerWidth > 768) {
            if (elementInGroup !== 3) setGroup(3)
        } else if (elementInGroup !== 2) setGroup(2)
    }

    // Запрос на получение всех новостних постов
    async function pullPosts()
    {
        let data = await Service.getAllPosts()
        if (data) setPosts(data)
    }

    // Метод создания нового поста
    async function createPost(newPost) {
        const url = Service.getServerUrl()
        const requestOptions = {
            method: 'POST',
            type: 'multipart/form-data',
            headers: {'Accept': 'application/json'},
            body: newPost
        }
        const response = await fetch(`${url}/Post`, requestOptions);
        const result = await response.json()
        console.log(result)
		setModel(false)
	}

    // Метод показа всей информаци о себе
    async function getMe() {
        const url = Service.getServerUrl()
        const token = Service.getToken()
        const requestOptions = {
            method: 'GET',
            headers: { 'Accept': 'application/json', 'Authorization': `bearer ${token}`}
        }
        const response = await fetch(url + '/User', requestOptions)
        const message = await response.json()
        setInfo(message)
        setInfoModel(true)
    }

    return (
        <div>
            <h1 className='welcome'>Home Panel</h1>
            <div style={{marginLeft: 20}}>
                <MyButton style={{margin: 10}} onClick={() => setModel(true)}>
                    Create Post
                </MyButton>
                <MyButton style={{margin: 10}} onClick={getMe}>
                    Get Me
                </MyButton>
                <Link to="/Chat">
                    <MyButton>Chat</MyButton>
                </Link>
            </div>
            <GroupedPosts posts={posts} elementInGroup={elementInGroup} />
            <MyModel  visible={modal} setVisible={setModel}>
				<PostForm create={createPost}/>
			</MyModel>
            <MyModel visible={infoModel} setVisible={setInfoModel}>
                <h5>User ID: {info['id']}</h5>
                <h5>Email: {info['email']}</h5>
                <h5>Name: {info['name']}</h5>
                <h5>Role: {info['role']}</h5>
                <h5>Token: {info['token']}</h5>
            </MyModel>
        </div>
    )
}

export default Home;