import envs from "./env.json";

const env = envs[process.env.NODE_ENV || "development"];

export default env; 