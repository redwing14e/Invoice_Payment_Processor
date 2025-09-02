import axios from "axios";

const apiBaseUrl = "https:localhost:5001"

const api = axios.create({
    baseURl: apiBaseUrl,
    timeout: 5000,
});

//api connection for validation
export const validation = async(accountNumber, invoiceDate) =>{
    var response = null;
    try {
        response = await api.post("api/Validate/" + accountNumber +"/"+invoiceDate);
    } catch (error){
        console.log("validation failed with error", error);
        throw error;
    }
    console.log(response.data.amountDue);



};
