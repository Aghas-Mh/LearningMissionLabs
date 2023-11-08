import React from "react";

// Сообщения, которие видны в чате
const MessageForm = ({messageForm, myEmail}) => 
{

    // Определяет местоположение сообщения.
    // Если сообщение от меня, то будет ровнятся к правому краю.
    function getStyle()
    {
        if (messageForm.sender === myEmail) return { textAlign: "right" }
        return { textAlign: 'left'}
    }

    // Определяет цвет сообщения. Зелений или серый.
    // Если сообщение от меня, то цвет будет зеленым.
    function getColor()
    {
        if (messageForm.sender === myEmail) return { backgroundColor: "rgb(178, 255, 102)" }
        return { backgroundColor: 'rgb(192, 192, 192)'}
    }

    return (
    <div style={getStyle()}>
        <a style={{...getColor(), borderRadius: 10, padding: 3}}>
            {messageForm.message}
        </a>
    </div>
    )
}

export default MessageForm;