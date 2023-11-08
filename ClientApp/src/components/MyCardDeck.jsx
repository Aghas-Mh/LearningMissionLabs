import React from 'react';
import { CardDeck } from 'reactstrap';
import MyCard from './MyCard';

const MyCardDeck = ({ posts }) => {
    if (posts == null) {
        return (<div></div>)
    }
    return (
        <CardDeck style={{margin: '20px'}}>
            {posts.map(post =>
                <MyCard post={post} key={post.id} />
            )}
        </CardDeck>
    )
}

export default MyCardDeck;