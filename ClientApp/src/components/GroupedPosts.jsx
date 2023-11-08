import React from "react"
import MyCardDeck from "./MyCardDeck"

const GroupedPosts = ({posts, elementInGroup}) => {
    function getFirstPosts(start, end) {
        if (start >= posts.length) return null
        if (end >= posts.length) {
            end = posts.length
        }
        let data = posts.slice(start, end);
        return data;
    }

    function getGroupedPosts() {
        let groupedList = []
        for (let start = 0, end = elementInGroup; start < posts.length; start += elementInGroup, end += elementInGroup)
        {
            let firstPosts = getFirstPosts(start, end, elementInGroup)
            if (firstPosts == null) break
            groupedList.push(firstPosts)
        }
        return groupedList
    }

    return (
        <div>
            {getGroupedPosts().map((postsGroup, index) =>
                <MyCardDeck posts={postsGroup} key={index} />
            )}
        </div>
    )
}

export default GroupedPosts;