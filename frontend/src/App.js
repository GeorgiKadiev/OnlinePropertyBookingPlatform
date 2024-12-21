import './App.css';
// import LandingPage from './app/LandingPage/LandingPage';
// import ResultsPage from './app/ResultsPage/ResultsPage';
// import NavBar from './components/NavBar/NavBar';
// import LoginForm from './components/LoginForm/LoginForm';
// import RegisterForm from './components/RegisterForm/RegisterForm';
import { Routes, Route } from "react-router-dom";


// import test from './components/test/test'

function App() {
  return (
    <div className="App">
     {/* <NavBar></NavBar> */}
     <RegisterForm/>
     {/* <LoginForm/> */}
     {/* <LandingPage></LandingPage> */}
     {/* <ResultsPage/> */}
    </div>
  );
}

export default App;
