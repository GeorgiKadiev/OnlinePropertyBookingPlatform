import React, { useState, useEffect } from "react";
import UserLanding from "../UserPages/Landing/Landing";
import OwnerHome from "../OwnerPages/OwnerHomePage/OwnerHome";
import AdminDashboard from "../AdminPages/AdminDashboard/AdminDashboard";
import { useLocation } from "react-router-dom";


// Utility function to fetch user ID
const fetchUserId = async (token) => {
  const response = await fetch("http://localhost:5076/api/user/get-user-id", {
    method: "GET",
    headers: {
      "Authorization": `Bearer ${token}`,  // Pass the token in the request header
    },
    credentials: "include", // Use credentials if cookies are required for auth
  });
  if (!response.ok) {
    throw new Error("Failed to fetch user ID");
  }
  return response.json();
};

// Utility function to fetch user details by ID
const fetchUserDetails = async (userId) => {
  userId = 17;
  const response = await fetch(`http://localhost:5076/api/user/${userId}`, {
    method: "GET",
  });
  if (!response.ok) {
    throw new Error("Failed to fetch user details");
  }
  return response.json();
};

export default function LandingPage() {
  const [userDetails, setUserDetails] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const location = useLocation();  // Get the state from the router


  // Fetch user details on component mount
  useEffect(() => {
    const { token } = location.state || {};  // Access the passed token

    if (!token) {
      console.error("No token found");
      return;
    }

    const loadUserDetails = async () => {
      try {
        // Step 1: Fetch User ID
        const { id: userId } = await fetchUserId(token);
        console.log("User ID:", userId);

        // Step 2: Fetch User Details
        const user = await fetchUserDetails(userId);
        console.log("User Details:", user);

        setUserDetails(user); // Update state with user details
      } catch (err) {
        console.error("Error:", err.message);
        setError(err.message);
      } finally {
        setLoading(false); // Set loading to false
      }
    };

    loadUserDetails();
  }, []);

  // Loading state
  if (loading) return <div>Loading...</div>;

  // Error state
  if (error) return <div>Error: {error}</div>;

  // Determine component to render based on role
  const { role } = userDetails || {};
  console.log("User Role:", role);

  switch (role) {
    case "Admin":
      return <AdminDashboard />;
    case "Customer":
      return <UserLanding />;
    case "PropertyOwner":
      return <OwnerHome />;
    default:
      return <div>Unknown role</div>;
  }
}
