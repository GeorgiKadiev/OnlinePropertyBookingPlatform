import "./App.css";
import { Routes, Route } from "react-router-dom";
import LandingPage from "./app/LandingPage/LandingPage";
import ResultsPage from "./app/UserPages/ResultsPage/ResultsPage";
import LoginForm from "./components/LoginForm/LoginForm";
import RegisterForm from "./components/RegisterForm/RegisterForm";
import HomePage from "./app/HomePage/HomePage";
import SuccessRegister from "./components/Success/SuccessRegister";
import Success from "./components/Success/GenericSuccess";
import PropertyPage from "./app/UserPages/PropertyPage/PropertyPage";
import PropertyForm from "./components/PropertyForm/PropertyForm";

function App() {
  return (
    <div className="App">
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/register" element={<RegisterForm />} />
        <Route path="/login" element={<LoginForm />} />
        <Route path="/landing-page" element={<LandingPage />} />
        <Route path="/results" element={<ResultsPage />} />
        <Route path="/successregister" element={<SuccessRegister />} />
        <Route path="/success" element={<Success />} />
        <Route path="/property/:id" element={<PropertyPage />} />
        <Route path="/cerate-property" element={<PropertyForm />} />
      </Routes>
    </div>
  );
}

export default App;
