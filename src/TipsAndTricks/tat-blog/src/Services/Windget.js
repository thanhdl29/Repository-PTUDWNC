import axios from "axios";

export async function getCategories(){
    try{
        const reponse = await
        axios.get(`https://localhost:7085/api/categories`);
        const data = reponse.data;
        if(data.isSuccess)
         return data.result;
        else
        return null;
}catch (error){
    console.log('Error', error.message);
    return null;
    
}
}