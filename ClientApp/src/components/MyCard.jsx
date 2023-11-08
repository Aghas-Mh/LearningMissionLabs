import React from 'react';
import { Card, CardBody, CardTitle, CardText, Button } from 'reactstrap';

const MyCard = ({ post }) => {
    let backColor = 'linear-gradient(to bottom, rgb(135, 139, 163), rgb(86, 82, 142), rgb(86, 82, 142))'
    let textColor = '#fff'

    return (
        <Card style={{width: '18rem', background: backColor}}>
            <img alt="Sample" src={`data:image/jpeg;base64,${post.imageContent ? post.imageContent.fileContents : null}`} />
            <CardBody>
                <CardTitle className="title" tag="h5" style={{ color: textColor }}>
                    {post.title}
                </CardTitle>
                <CardText style={{ color: textColor }}>
                    {post.description}
                </CardText>
                <Button>
                    Select
                </Button>
            </CardBody>
        </Card>
    )
}

export default MyCard;