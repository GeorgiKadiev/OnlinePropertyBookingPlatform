import NavBar from "../../../components/NavBar/NavBar";
import EstateReviews from "../../../components/EstateReviews/EstateReviews";

export default function ReviewsPage({token}){
    return (
        <div>
          <NavBar token={token}/>
          <EstateReviews />
        </div>
      );
};
