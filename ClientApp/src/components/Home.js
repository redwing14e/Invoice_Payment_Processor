import './Home.css'
import React, { Component, useState, useCallback, process, env} from 'react';
import { validation } from '../api/Api';
import { Alert } from 'reactstrap';
import {loadStripe} from '@stripe/stripe-js';
import {
  EmbeddedCheckoutProvider,
  EmbeddedCheckout
} from '@stripe/react-stripe-js';


const stripePromise = loadStripe(process.env.REACT_APP_STRIPE_PUBLISHABLE_KEY);

export class Home extends Component {
  static displayName = Home.name;
  

  render () {
    return (
      <div>
        
        
        <div className='payment-box'>
           <PaymentForm /> 
        </div>
      </div>
    );
  }
}

function PaymentForm(){
   
  const [accountNumber, setAccountNumber] = useState("");
  const [invoiceDate, setInvoiceDate] = useState("");
  const [errorVisible, setErrorVisible] = useState(false);
  const [paymentVisible, setPaymentVisible] = useState(false);
  const [options, setOptions] = useState(null);
  

  
  //call the validation api with provided accountNumber and invoiceDate
  const handleSubmit = async (e) => {
    e.preventDefault();
    try{
      //check that a invoiceDate was given
      if (invoiceDate == ""){
        throw new Error;
      }
      var response = await validation(accountNumber, invoiceDate);
      setErrorVisible(false);
      
      //creates the checkout session
      setOptions({fetchClientSecret});

      //show the checkout session
      setPaymentVisible(true);

    } catch(error){
      console.log("submit error: " + error);
      setErrorVisible(true);
    }
    
  }

  
  // call the backend for a new checkout session
  const fetchClientSecret = useCallback(() => {
    // Create a Checkout Session
    return fetch("api/Pay/create-checkout-session/" + accountNumber, {
      method: "POST",
    })
      .then((res) => res.json())
      .then((data) => data.clientSecret);
  }, [accountNumber]);
  

  return(
    <div style={{display:'flex', justifyContent:'space-between', width:'80vw', maxWidth:'1300px'}}>
      <div>
        <h1>Pay Invoice</h1>
        <form className='payment-form' onSubmit={handleSubmit}>
          <div>
            <label htmlFor='accountNumber' className='payment-form-item'>Account Number: </label>
            <input
              type='text' 
              name='accountNumber' 
              id='accountNumber' 
              onChange={(e) => setAccountNumber(e.target.value)} 
              value={accountNumber} 
              className='payment-form-item' 
              placeholder='ACC-12345'
            />
          </div>
          <div>
            <label htmlFor='invoiceDate' className='payment-form-item'>Invoice Date: </label>
            <input 
              type='date' 
              name='invoiceDate' 
              id='invoiceDate' 
              onChange={(e) => setInvoiceDate(e.target.value)} 
              value={invoiceDate} 
              className='payment-form-item'
            />
          </div>
          <div>
            <input type='submit' value='Find My Bill' className='payment-form-submit'></input>
          </div>
        </form>
        <Alert color="danger" isOpen={errorVisible} toggle={(e) => setErrorVisible(false)}>
          Invalid account number or Invoice Date
        </Alert>
      </div>
      <div>
        {paymentVisible && 
        <div id="checkout">
          <EmbeddedCheckoutProvider
            stripe={stripePromise}
            options={options}
          >
            <EmbeddedCheckout />
          </EmbeddedCheckoutProvider>
        </div>}
      </div>
    </div>
  )
}
