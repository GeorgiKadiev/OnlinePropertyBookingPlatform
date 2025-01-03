import "./App.css";
import LandingPage from "./app/LandingPage/LandingPage";
import ResultsPage from "./app/ResultsPage/ResultsPage";
import LoginForm from "./components/LoginForm/LoginForm";
import RegisterForm from "./components/RegisterForm/RegisterForm";
import { Routes, Route } from "react-router-dom";
import HomePage from "./app/HomePage/HomePage";
import SuccessRegister from "./components/Success/SuccessRegister";
import Success from "./components/Success/GenericSuccess";

import NavBar from "./components/NavBar/NavBar";

function App() {
  return (
    <div className="App">
      <NavBar />
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/register" element={<RegisterForm />} />
        <Route path="/login" element={<LoginForm />} />
        <Route path="/landing-page" element={<LandingPage />} />
        <Route path="/results" element={<ResultsPage />} />
        <Route path="/successregister" element={<SuccessRegister />} />
        <Route path="/success" element={<Success />} />
      </Routes>
    </div>
  );
}

export default App;
