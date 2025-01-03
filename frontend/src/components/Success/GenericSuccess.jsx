import React from "react";
import {  Link } from "react-router-dom";

function Success() {
  return (
    <div>
      <h1>Successful!</h1>
      <Link to="/">Home Page</Link>
    </div>
  );
}

export default Success;