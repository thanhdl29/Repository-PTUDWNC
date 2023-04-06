import React, { useEffect, useState } from "react";
import PostItem from "../Components/Postltem";
import { getPosts } from "../Services/BlogRepository";
const Index = () =>{
    const [postList, setPostList] = useState([]);
    useEffect(() =>{
        document.title = 'Trang chủ';

        getPosts().then(data => {
            if(data)
                setPostList(data.item);
            else
                setPostList([]);
        })
    }, []);
    // return(
    //     <h1>
    //         Đây là trang chủ
    //     </h1>
    // );
    if(postList.length > 0){
        return(
            <div className="p-4">
                {postList.map(item => {
                    return(
                        <PostItem postItem={item}></PostItem>
                    );
                })};
            </div>
        );
       
    } else return(
            <></>
        );
}
export default Index;