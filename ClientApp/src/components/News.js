import React, { useEffect, useMemo, useState } from 'react';
import GroupedPosts from './GroupedPosts';
import { Service } from '../Service';

const News = () =>
{
    const [posts, setPosts] = useState([])
    const [elementInGroup, setGroup] = useState(4)

    useMemo(() => {
        pullPosts()
    }, [])

    useEffect(() => {
        window.addEventListener('resize', windowSizeHandler)
        return () => {
            window.removeEventListener('resize', windowSizeHandler)
        }
    }, [elementInGroup])

    async function windowSizeHandler() {
        if (window.innerWidth > 992) {
            if (elementInGroup !== 4) setGroup(4)
        } else if (window.innerWidth > 768) {
            if (elementInGroup !== 3) setGroup(3)
        } else if (elementInGroup !== 2) setGroup(2)
    }

    
    async function pullPosts()
    {
        let data = await Service.getAllPosts()
        if (data) setPosts(data)
    }

    return (
        <div>
            <h1 className='welcome'>Welcome to Learning Mission Labs!</h1>
            <GroupedPosts posts={posts} elementInGroup={elementInGroup}/>
        </div>
    );
}

export default News;