import './App.css';
import LandingPage from './app/LandingPage/LandingPage';
import NavBar from './components/NavBar/NavBar';

// import test from './components/test/test'

function App() {
  return (
    <div className="App">
     <NavBar></NavBar>
     <LandingPage></LandingPage>
    </div>
  );
}

export default App;
