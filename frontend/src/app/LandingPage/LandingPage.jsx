import React, { useState, useEffect } from "react";
import { useLocation } from "react-router-dom";
import { useSelector } from "react-redux";
import UserLanding from "../UserPages/Landing/Landing";
import OwnerHome from "../OwnerPages/OwnerHomePage/OwnerHome";
import AdminDashboard from "../AdminPages/AdminDashboard/AdminDashboard";

const fetchUserId = async (token) => {
  const response = await fetch("http://localhost:5076/api/user/get-user-id", {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
    },
    credentials: "include",
  });
  if (!response.ok) {
    throw new Error("Failed to fetch user ID");
  }
  return response.json();
};

const fetchUserDetails = async (userId, token) => {
  console.log(userId);
  const response = await fetch(`http://localhost:5076/api/user/${userId}`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
    },
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
  const location = useLocation();
  const token = useSelector((state) => state.token);

  useEffect(() => {
    if (!token) {
      console.error("No token found");
      return;
    }

    const loadUserDetails = async () => {
      try {
        const { userId: userId } = await fetchUserId(token);
        console.log("User ID:", userId);

        const user = await fetchUserDetails(userId, token);
        console.log("User Details:", user);

        setUserDetails(user);
      } catch (err) {
        console.error("Error:", err.message);
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    loadUserDetails();
  }, [location.state]);

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
    case "EstateOwner":
      return <OwnerHome />;
    default:
      return <div>Something went wrong</div>;
  }
}
