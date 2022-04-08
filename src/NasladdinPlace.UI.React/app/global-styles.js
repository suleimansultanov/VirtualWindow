import { injectGlobal } from 'styled-components';

/* eslint no-unused-expressions: 0 */
injectGlobal`
  html,
  body {
    height: 100%;
    width: 100%;
  }

  .scaling-container {
    position: absolute;
    height: auto;
    width: 100%;
    top: 50%;
    transform: translateY(-50%);
  }

  .scaling-div {
    height: 100%; 
    width: 100%; 
    padding: 0 10% 0 10%;
    left: 0; 
    top: 0;
  }

  #app {
    padding: 15px;
    min-height: 100%;
    min-width: 100%;
  }
  
  .welcome-screen {
    position: absolute;
    left: 15px;
    top: 15px;
    right: 15px;
    bottom: 15px;
    color: #333333;
  }

  .welcome-screen__qr-code {
    display: table;
    margin: 0 auto;
  }

  .welcome-screen__help {
    color: white;
    font-size: 44px;
    font-weight: bold;
    height: 200px;
  }
  
  .welcome-screen__help p {
    margin: 0 auto;
    display: table;
    text-transform: uppercase;
  }

  .gradient-shadow {
    box-shadow: inset 3px 3px 5px 6px #ccc;
  }

  .w50 {
    position: absolute;
    width: 50%;
    height: 100%;
  }

   .t-n-w {
    white-space: nowrap;
   }

  .h50 {
    position: absolute;
    height: 45%;
    width: 100%;
  }

  .l0 {
    left: 80px;
  }

  .r0  {
     right: 0;
  }

  .b0{
    bottom: 40px;
  }

  .t0 {
    top: 0;
  }

  .bonus-text {
    font-size: 235px;
    display:inline-block
    color: #EF0330;
    margin-left:-10px;
    line-height: 1;
  }

  .bonus-image {
    display:inline-block
    margin: 5px;
    height: 155px;
    padding-left: 35px;
  }
  
  .bonus-image img {
    vertical-align: unset;
    height: inherit;
  }

  .f-b {
    font-size: 60px;
    line-height: 1;
  }

  .f-m {
    font-size: 44px;
  }
  
  .bold {
    font-weight: bold;
  }

  .center-absolute {
    position: absolute;
    transform: translate(-50%, -50%);
    top: 40%;
    left: 50%;
  }

  .mt-2 {
    margin-top: 25px;
  }

  .mt-3 {
    margin-top: 45px;
  }
  
  .mt-4 {
    margin-top: 65px;
  }

  .p-t-3 {
    padding-top: 45px;
  }

  .p-t-2 {
    padding-top: 30px;
  }

  .p0 {
    padding: 0;
  }

  .p-l-1 {
    padding-left: 5px;
  }

  .qr-timer {
    width:100%;
    height: 100%;
  }
  
  .qr-timer svg {
      position:absolute;
      width:100%;
      height: 80%;
      top:-20%;
  }

  .logo-s {
    height:45px;
  }

  .icon {
    margin-left: 35px;
  }
  
  .text-timer {
    position: relative;
    top: 50%;
    transform: translateY(30%);
  }

  .discount-text {
    line-height: 1.2;
  }

  .mobile-app-update-text {
    font-size: 45px;
    line-height: 1.5;
  }

  @media only all and ( min-width: 1366px ) and (max-width: 1980px){
    @media only all and ( min-width: 1600px ){
      .center-absolute {
        left: 30% !important;
      }
    }

    @media only all and ( min-height: 960px ){
      .center-absolute {
        left: 50%;
        margin-left:68px;
      }
    }
  }
  
  @media only all and ( max-height: 960px ) {
    @media only all and ( min-width: 1366px ){
      .center-absolute {
        left: 40% !important;
        margin-left:0px !important;
      }

      r0 {
        right: 0px !important;
      }
    }
   
    .center-absolute {
      left: 50%;
      margin-left:70px;
    }

    .b0{
      bottom: 20px;
    }
   
    .bonus-text {
      font-size: 200px;
      color: #EF0330;
      line-height: 1;
    }
  
    .bonus-image {
      margin: 5px;
      padding-left:30px;
      height: 140px;
    }
  
    .bonus-image img {
      vertical-align: unset;
      height: inherit;
    }
  
    .f-b {
      font-size: 55px;
      line-height: 1;
    }

    .mobile-app-update-text {
      font-size: 40px;
    }
  
    .f-m {
      font-size: 34px;
    }
  
    .bold {
      font-weight: bold;
    }
  
    .mt-2 {
      margin-top: 15px;
    }

    .mt-3 {
      margin-top: 35px;
    }

    .mt-4 {
      margin-top: 75px;
    }

    .p-t-3 {
      padding-top: 35px;
    }
  
    .p-t-2 {
      padding-top: 20px;
    }
  
    .p0 {
      padding: 0;
    }
  
    .p-l-1{
      padding-left: 5px;
    }

    .logo-s {
      height: 30px;
    }
    
    .qr-s svg{
      height: 300px;
      weight: 300px;
    }

    .logo-s {
      height:35px;
    }
  
    .icon {
      margin-left: 30px;
    }

    .qr-m-l {
      margin-left: -29px;
    }
  }

  @media only all and (max-width: 1366px) { 
      @media only all and ( min-height: 960px ){
        .center-absolute {
          margin-left: 75px !important;
        }
      }
      
      .b0{
        bottom: 30px;
      }

      .center-absolute {
        left: 40%;
      }

      .bonus-text {
        font-size: 180px;
        color: #EF0330;
        line-height: 1;
      }
    
      .bonus-image {
        margin: 5px;
        padding-left:25px;
        height: 125px;
      }
    
      .bonus-image img {
        vertical-align: unset;
        height: inherit;
      }
    
      .f-b {
        font-size: 45px;
        line-height: 1;
      }

      .mobile-app-update-text {
        font-size: 35px;
      }
    
      .f-m {
        font-size: 28px;
      }
    
      .bold {
        font-weight: bold;
      }

      .text-timer {
        transform: translateY(35%);
      }
    
      .mt-2 {
        margin-top: 15px;
      }

      .mt-3 {
        margin-top: 35px;
      }

      .mt-4 {
        margin-top: 75px;
      }
    
      .p-t-3 {
        padding-top: 65px;
      }
    
      .p-t-2 {
        padding-top: 20px;
      }
    
      .p0 {
        padding: 0;
      }
    
      .p-l-1{
        padding-left: 5px;
      }
  
      .logo-s {
        height: 35px;
      }
      
      .qr-s svg{
        height: 300px;
        weight: 300px;
      }
      
      .qr-m-l {
        margin-left: -20px;
      }

      .center-absolute {
        margin-left: 50px !important;
      }

      .icon img {
        height: 40px;
      }
  
      .icon {
        margin-left: 25px;
      }
    }

    @media only all and ( max-height: 820px ) {
      .b0{
        bottom: 10px;
      }
     
      .bonus-text {
        font-size: 160px;
        color: #EF0330;
        line-height: 1;
      }
    
      .bonus-image {
        margin: 5px;
        padding-left:15px;
        height: 110px;
      }
    
      .bonus-image img {
        vertical-align: unset;
        height: inherit;
      }
    
      .f-b {
        font-size: 40px;
        line-height: 1;
      }
    
      .f-m {
        font-size: 28px;
      }
   
      .mobile-app-update-text {
        line-height: 1.5;
        font-size: 35px;
      }

      .bold {
        font-weight: bold;
      }
    
      .mt-2 {
        margin-top: 15px;
      }

      .mt-3 {
        margin-top: 35px;
      }
      
      .mt-4 {
        margin-top: 85px;
      }
    
      .p-t-3 {
        padding-top: 35px;
      }
    
      .p-t-2 {
        padding-top: 20px;
      }
    
      .p0 {
        padding: 0;
      }
    
      .p-l-1{
        padding-left: 5px;
      }
  
      .logo-s {
        height: 25px;
      }
      
      .qr-s svg{
        height: 250px;
        weight: 250px;
      }

      .logo-s {
        height:30px;
      }
    
      .icon {
        margin-left: 20px;
      }

      .qr-m-l {
        margin-left: -50px;
      }
    }
`;
