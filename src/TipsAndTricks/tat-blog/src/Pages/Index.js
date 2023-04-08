import React, { useEffect, useState } from "react";
import PostItem from "../Components/Postltem";
import { useLocation } from "react-router-dom";
import Pager from "../Components/Pager";
import { getPosts } from "../Services/BlogRepository";
const Index = () =>{
    const [postList, setPostList] = useState([]);
    const [metadata, setMetadata] = useState([]);
    function useQuery(){
        const{serch} = useLocation();
        return React.useMemo(() => new URLSearchParams(serch), [serch]);
    }
    let query = useQuery(),
        k = query.get('k') ?? '',
        p = query.get('p') ?? 1,
        ps = query.get('ps') ?? 10;

    useEffect(() =>{
        document.title = 'Trang chủ';

        getPosts().then(data => {
            if(data){
                setPostList(data.item);
                setMetadata(data.metadata);
            }
            else
                setPostList([]);
        })
    }, [k,p,ps]);
    useEffect(() => {
        window.scrollTo(0,0);

    }, [postList]);
    if(postList.length > 0){
        return(
            <div className="p-4">
                {postList.map((item, index) => {
                    return(
                        <PostItem postItem={item} key={index}></PostItem>
                    );
                })};
                <Pager postquery={{'keyword':k}} metadata={metadata}/>
            </div>
        );
       
    } else return(
            <></>
        );
}
export default Index;