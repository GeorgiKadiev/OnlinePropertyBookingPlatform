import './App.css';
import LandingPage from './app/LandingPage/LandingPage';
import ResultsPage from './app/ResultsPage/ResultsPage';
import NavBar from './components/NavBar/NavBar';
import RegisterForm from './components/RegisterForm/RegisterForm';

// import test from './components/test/test'

function App() {
  return (
    <div className="App">
     {/* <NavBar></NavBar> */}
     <RegisterForm/>
     {/* <LandingPage></LandingPage> */}
     {/* <ResultsPage/> */}
    </div>
  );
}

export default App;
