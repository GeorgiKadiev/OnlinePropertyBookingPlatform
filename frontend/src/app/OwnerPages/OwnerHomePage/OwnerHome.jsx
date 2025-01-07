import NavBar from "../../../components/NavBar/NavBar";
import PropertyList from "../../../components/PropertyList/PropertyList";

export default function OwnerHome({token}){
    return (
        <div>
          <NavBar token={token}/>
          <PropertyList />
        </div>
      );
};
