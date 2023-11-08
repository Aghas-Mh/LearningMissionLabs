import React from "react";

const ChatUserModel = ({user, index, callback}) => {
    
    // На данный момент не работает. (всегда оффлайн)
    const getStatus = () => {
        if (user.status)
        {
            return 'Online'
        }
        return 'Offline'
    }

    return (
        <div onClick={() => callback(index)} style={{border: '1px solid', borderRadius: 5, marginTop: 5, paddingLeft: 2}}>
            {user.email}<br/>
            {user.name} | Status: {getStatus()}
        </div>
    )
}

export default ChatUserModel