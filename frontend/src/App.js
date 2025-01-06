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
import CreateProperyPage from "./app/OwnerPages/CreatePropertyPage/CreateProperyPage";
import ReseravtionsPage from "./app/OwnerPages/ReservationsPage/ReservationsPage";
import ForgotPassForm from "./components/ForgotPassForm/ForgotPassForm";

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
        <Route path="/create-property" element={<CreateProperyPage />} />
        <Route path="/owner-reservations" element={<ReseravtionsPage />} />
        <Route path="/forgot-password" element={<ForgotPassForm />} />

      </Routes>
    </div>
  );
}

export default App;
