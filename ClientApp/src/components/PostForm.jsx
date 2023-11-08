import React, { useState } from "react";
import MyButton from "./UI/button/MyButton";
import MyInput from "./UI/input/MyInput";

const PostForm = ({create}) => {
	const [post, setPost] = useState({title: '', description: '', image: ''})
	const [formData, setFormData] = useState(null)

	const addNewPost = (e) => {
		e.preventDefault()
		create(formData)
	}

	const setImage = (files) => {
		console.log(files[0])
		const form = new FormData()
		form.append('Title', post.title)
		form.append('Description', post.description)
		form.append('Image', files[0])
		setFormData(form)
	}

	return (
        <div>
            <form>
				<MyInput 
					value={post.title}
					type='text'
					placeholder='Title'
					onChange={e => setPost({...post, title: e.target.value})}
				/>

				<MyInput
					value={post.description}
					type='text'
					placeholder='Description'
					onChange={e => setPost({...post, description: e.target.value})}
				/>

				<MyInput
					onChange={e => setImage(e.target.files)}
					accept="image/*"
					type="file"
				/>

				<MyButton onClick={addNewPost}>Create Post</MyButton>
			</form>
        </div>
    )
}

export default PostForm;