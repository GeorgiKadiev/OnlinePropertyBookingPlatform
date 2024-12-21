import './App.css';
import LandingPage from './app/LandingPage/LandingPage';
import ResultsPage from './app/ResultsPage/ResultsPage';
import NavBar from './components/NavBar/NavBar';
import LoginForm from './components/LoginForm/LoginForm';
import RegisterForm from './components/RegisterForm/RegisterForm';
import { Routes, Route } from "react-router-dom";
import HomePage from './app/HomePage/HomePage';

function App() {
  return (
    <div className="App">
     <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/register" element={<RegisterForm />} />
        <Route path="/login" element={<LoginForm />} />
      </Routes>
    </div>
  );
}

export default App;
