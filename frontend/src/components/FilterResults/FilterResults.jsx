import React, { useState } from "react";
import Filters from "../Filters/Filetrs";

const EstateList = () => {
  const [estates, setEstates] = useState([]);

  const fetchFilteredEstates = async (filters) => {
    try {
      const response = await fetch("http://localhost:5076/api/estate/filter", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(filters),
      });

      if (!response.ok) {
        throw new Error("Failed to fetch estates");
      }

      const data = await response.json();
      setEstates(data);
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <div>
      <Filters onFilterSubmit={fetchFilteredEstates} />
      <div>
        <h2>Filtered Estates</h2>
        {estates.length > 0 ? (
          <ul>
            {estates.map((estate) => (
              <li key={estate.id}>
                <h3>{estate.title}</h3>
                <p>{estate.description}</p>
                <p>Location: {estate.location}</p>
                <p>Price: ${estate.pricePerNight}</p>
                <p>Amenities: {estate.amenities?.join(", ")}</p>
              </li>
            ))}
          </ul>
        ) : (
          <p>No estates found.</p>
        )}
      </div>
    </div>
  );
};

export default EstateList;
